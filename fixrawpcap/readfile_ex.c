#include <stdio.h>
#include <pcap.h>



#define LINE_LEN 16
unsigned char maclayer[]= {0x0a, 0xe4, 0x84, 0x29, 0x5f, 0x0c, 0x00, 0x15, 0x58, 0xc2, 0x30, 0x53, 0x08, 0x00};

int main(int argc, char **argv)
{
	pcap_t *fp=NULL;
	pcap_t *writefp=NULL;
	char errbuf[PCAP_ERRBUF_SIZE];
	struct pcap_pkthdr *header;
	const u_char *pkt_data;
	u_int i=0;
	int res;
	pcap_dumper_t *fdumper=NULL;
	struct pcap_pkthdr newheader;
	u_char *newpkt_data= NULL;
	char *outputfilename= NULL;
	
	
	if(argc != 2)
	{	
		printf("usage: %s filename", argv[0]);
		return -1;

	}

	outputfilename= (char*)calloc(1, strlen(argv[1])+10);
	if (outputfilename == NULL){
		printf("Error allocating memory\n");
		return -1;
	}

	strcpy(outputfilename, argv[1]);
	
	strcat(outputfilename, "_e.pcap");

	printf("%s\n", outputfilename);

	/* Open the capture file */
	if ((fp = pcap_open_offline(argv[1],			// name of the device
						 errbuf					// error buffer
						 )) == NULL)
	{
		fprintf(stderr,"\nUnable to open the file %s.\n", argv[1]);
		return -1;
	}

	printf("linktype= %d\n" , pcap_datalink(fp));

	if ( pcap_datalink(fp) != 12){
		printf("This file does not need any alteration.\n");
		pcap_close(fp);
		return 0;
	}

	writefp= pcap_open_dead(DLT_EN10MB, 65535);

	fdumper = pcap_dump_open(writefp, outputfilename);
	
	/* Retrieve the packets from the file */
	while((res = pcap_next_ex(fp, &header, &pkt_data)) >= 0)
	{
		/* print pkt timestamp and pkt len */
		//printf("header->caplen= %ld header->len= %ld\n", header->caplen, header->len);	
		memset(&newheader, 0, sizeof(struct pcap_pkthdr));
		
		newheader.ts.tv_sec= header->ts.tv_sec;
		newheader.ts.tv_usec= header->ts.tv_usec;
		newheader.caplen= header->caplen;
		newheader.len= header->len;

		newpkt_data= (u_char *)calloc(1, newheader.caplen + sizeof(maclayer));
		if (newpkt_data == NULL){
			printf("Error in allocating memory\n");
			break;
		}

		memcpy(newpkt_data, maclayer, sizeof(maclayer));
		memcpy(&newpkt_data[sizeof(maclayer)], pkt_data, header->caplen);

		newheader.caplen+= sizeof(maclayer);
		newheader.len+= sizeof(maclayer);
		
		pcap_dump((pcap_dumper_t *)fdumper, &newheader, newpkt_data);
		free(newpkt_data);
		
		//printf("\n\n");		
		//break;
	}
	
	
	if (res == -1)
	{
		printf("Error reading the packets: %s\n", pcap_geterr(fp));
	}
	
	pcap_dump_close(fdumper);
	pcap_close(fp);
	return 0;
}

